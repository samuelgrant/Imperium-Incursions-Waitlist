import React, { Component } from 'react';
import { render } from 'react-dom';
import ActiveBans from './Components/BanManagement/ActiveBans';
import BanInfo from './Components/BanManagement/BanInfo';

const baseUri = "/admin/bans";

export default class BanManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            bans: null,
            admin: false,
            banIndex: null,
            key: 0
        }
    }

    componentDidMount() {
        this.getData();
    }

    getData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/active`,
        }).done((data) => {
            this.setState({
                bans: data.bans,
                admin: data.admin,
                banIndex: null,
                key: this.state.key + 1
            });
        }).fail((err) => {
            console.error(`[React/Bans] @getData - Error retrieving active bans`, err.responseText);
        })
    }

    selectedBan() {
        if (this.state.bans && this.state.bans[this.state.banIndex])
            return this.state.bans[this.state.banIndex];

        return null;
    }
    
    setBanIndex(index) {
        this.setState({ banIndex: index});
    }

    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-lg-8 col-md-12">
                        <ActiveBans bans={this.state.bans}
                            u={this.getData.bind(this)}
                            setIndex={this.setBanIndex.bind(this)}
                            admin={this.state.admin}
                            baseUri={baseUri} />
                    </div>

                    <div className="col-lg-4 col-md-12">
                        <BanInfo
                            u={this.getData.bind(this)}
                            selectedBan={this.selectedBan()}
                            setIndex={this.setBanIndex.bind(this)}
                            admin={this.state.admin}
                            baseUri={baseUri}
                            key={this.state.key} />
                    </div>
                </div>
            </div>
        );
    }
}

if (document.getElementById('bans'))
    render(<BanManagement />, document.getElementById('bans'));